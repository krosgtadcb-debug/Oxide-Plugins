using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("CreativeMod", "OxyMonster", "1.0.0")]
    [Description("Permite a los jugadores construir, mejorar, craftear y experimentar libremente sin limitaciones de recursos.")]
    public class CreativeMod : RustPlugin
    {
        #region Configuración

        private PluginConfig _config;

        protected override void LoadDefaultConfig() => _config = new PluginConfig();

        protected override void SaveConfig() => Config.WriteObject(_config, true);

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<PluginConfig>();
            if (_config == null)
            {
                _config = new PluginConfig();
                SaveConfig();
            }
        }

        private class PluginConfig
        {
            [JsonProperty(PropertyName = "Activar construcción gratis")]
            public bool EnableFreeBuild = true;

            [JsonProperty(PropertyName = "Activar mejora gratis")]
            public bool EnableFreeUpgrade = true;

            [JsonProperty(PropertyName = "Activar crafteo gratis")]
            public bool EnableFreeCraft = true;

            [JsonProperty(PropertyName = "Activar crafteo instantáneo")]
            public bool EnableInstantCraft = true;

            [JsonProperty(PropertyName = "Estabilidad de construcción al 100%")]
            public bool EnableFullStability = true;

            [JsonProperty(PropertyName = "Permiso de uso (vacío = todos pueden usarlo)")]
            public string UsePermission = "";
        }

        #endregion

        #region Permisos e Inicialización

        private void Init()
        {
            if (!string.IsNullOrEmpty(_config.UsePermission))
            {
                permission.RegisterPermission(_config.UsePermission, this);
            }

            Puts("CreativeMod cargado - Construcción/Mejora/Crafteo gratis activados");
        }

        private bool CanUse(BasePlayer player)
        {
            if (string.IsNullOrEmpty(_config.UsePermission)) return true;
            return permission.UserHasPermission(player.UserIDString, _config.UsePermission);
        }

        #endregion

        #region Construcción Gratis - Bloquear consumo de recursos

        // Intercepta al construir y devuelve los recursos consumidos
        private object OnEntityBuilt(Planner plan, GameObject go)
        {
            if (!_config.EnableFreeBuild) return null;
            if (plan == null || plan.GetOwnerPlayer() == null) return null;
            if (!CanUse(plan.GetOwnerPlayer())) return null;

            var player = plan.GetOwnerPlayer();
            var item = plan.GetItem();
            if (item != null)
            {
                player.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
                plan.SetItem(null);
            }

            return null;
        }

        #endregion

        #region Crafteo Gratis - Bloquear consumo de materiales

        // Intercepta antes de que empiece el crafteo
        private object OnItemCraft(ItemCraftTask task, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData)
        {
            if (!_config.EnableFreeCraft) return null;
            if (owner == null || !CanUse(owner)) return null;

            // Si el crafteo instantáneo está activado, se entrega directamente
            if (_config.EnableInstantCraft)
            {
                var blueprint = task.blueprint;
                if (blueprint != null)
                {
                    var targetItem = blueprint.targetItem;
                    int amount = task.amount * targetItem.amountToCreate;

                    var item = ItemManager.Create(targetItem, amount);
                    if (item != null)
                    {
                        owner.GiveItem(item, BaseEntity.GiveItemReason.Crafted);
                    }

                    // Cancela la tarea de crafteo original
                    return false;
                }
            }

            // Crafteo normal: rellena los materiales faltantes
            foreach (var ingredient in task.blueprint.ingredients)
            {
                int amountNeeded = (int)ingredient.amount * task.amount;
                var items = owner.inventory.FindItemsByItemID(ingredient.itemid);
                int total = 0;

                foreach (var item in items)
                {
                    if (total >= amountNeeded) break;
                    int toTake = Mathf.Min(item.amount, amountNeeded - total);
                    total += toTake;
                }

                if (total < amountNeeded)
                {
                    var giveItem = ItemManager.CreateByItemID(ingredient.itemid, amountNeeded - total);
                    owner.GiveItem(giveItem, BaseEntity.GiveItemReason.Crafted);
                }
            }

            return null;
        }

        // Devolución de materiales al terminar el crafteo (alternativa)
        private void OnItemCraftFinished(ItemCraftTask task, Item item, ItemCrafter crafter)
        {
            if (!_config.EnableFreeCraft) return;
            if (crafter == null || crafter.owner == null) return;
            if (!CanUse(crafter.owner)) return;

            foreach (var ingredient in task.blueprint.ingredients)
            {
                int amount = (int)ingredient.amount * task.amount;
                var refundItem = ItemManager.CreateByItemID(ingredient.itemid, amount);
                if (refundItem != null)
                {
                    crafter.owner.GiveItem(refundItem, BaseEntity.GiveItemReason.Crafted);
                }
            }
        }

        #endregion

        #region Mejora Gratis - Bloquear consumo al mejorar

        private object OnStructureUpgrade(BuildingBlock block, BasePlayer player, BuildingGrade.Enum grade)
        {
            if (!_config.EnableFreeUpgrade) return null;
            if (player == null || !CanUse(player)) return null;

            var items = block.blockDefinition.grades[(int)grade].costToBuild;
            if (items != null)
            {
                foreach (var itemAmount in items)
                {
                    var refundItem = ItemManager.CreateByItemID(itemAmount.itemid, (int)itemAmount.amount);
                    if (refundItem != null)
                    {
                        player.GiveItem(refundItem, BaseEntity.GiveItemReason.Crafted);
                    }
                }
            }

            return null;
        }

        #endregion

        #region Estabilidad de Construcción

        private void OnEntityBuilt(Planner plan, GameObject go, BuildingBlock block)
        {
            if (!_config.EnableFullStability) return;
            if (block == null) return;

            block.grade = block.grade;
        }

        private object OnBuildingBlockDemolish(BuildingBlock block, BasePlayer player)
        {
            if (!_config.EnableFullStability) return null;
            return false; // Bloquea la demolición
        }

        #endregion

        #region Comandos de Consola

        [ConsoleCommand("creativemod.toggle")]
        private void CmdToggle(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (player == null || !player.IsAdmin) return;

            _config.EnableFreeBuild = !_config.EnableFreeBuild;
            _config.EnableFreeCraft = !_config.EnableFreeCraft;
            _config.EnableFreeUpgrade = !_config.EnableFreeUpgrade;
            SaveConfig();

            Puts($"Estado de CreativeMod: Construcción={_config.EnableFreeBuild}, Crafteo={_config.EnableFreeCraft}, Mejora={_config.EnableFreeUpgrade}");
        }

        #endregion
    }
}
