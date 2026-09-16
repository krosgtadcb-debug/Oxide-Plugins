Rust Plugins Collection - Devblog 133 & More
=============================================

Descripcion
-----------
Coleccion de plugins para Rust desarrollados para el Devblog 133 y
versiones posteriores. Incluye fixes de exploits, modos creativos y
utilidades de administracion para servidores.


Plugins Disponibles
-------------------

1. AntiCraftExploit
   Bloquea el exploit que permite craftear sin recursos ni receta
   usando el comando craft.add en versiones vulnerables.

   Hooks: OnServerCommand, CanCraft
   Comandos: anticraft.status, anticraft.kick
   Config: oxide/config/AntiCraftExploit.json

2. CreativeMod
   Modo creativo completo para builders, admins y servidores de
   construccion.

   Funciones: construccion gratis, crafteo gratis, mejora gratis,
   estabilidad al 100 por ciento.
   Comandos: creativemod.toggle
   Config: oxide/config/CreativeMod.json


Requisitos
----------
- Rust Server Devblog 133 o superior
- Oxide/uMod 2.0.2622 o superior (para Devblog 133)
- .NET Framework 4.6.1 o superior


Instalacion
-----------
1. Descarga el plugin desde la carpeta /plugins
2. Sube el archivo .cs a /oxide/plugins/ en tu servidor
3. Espera la compilacion automatica o reinicia el servidor
4. Verifica la consola para confirmar la carga


Configuracion
-------------
Cada plugin genera su propio archivo en /oxide/config/

Ejemplo AntiCraftExploit.json:
{
  "Registrar intentos de exploit en consola": true,
  "Expulsar al jugador que intente el exploit": false,
  "Mensaje al jugador bloqueado": "Este comando ha sido bloqueado."
}

Ejemplo CreativeMod.json:
{
  "Activar construccion gratis": true,
  "Activar mejora gratis": true,
  "Activar crafteo gratis": true,
  "Activar crafteo instantaneo": true,
  "Estabilidad de construccion al 100%": true,
  "Permiso de uso (vacio = todos pueden usarlo)": ""
}

Para recargar sin reiniciar:
oxide.reload NombreDelPlugin


Compatibilidad
--------------
Devblog 133 (Oct 2016)     - Soporte completo (Oxide 2.0.2622+)
Devblog 134-150            - Soporte completo
Devblog 151-200            - Soporte parcial
Versiones modernas         - Soporte limitado (craft.add ya no
                             permite craftear gratis)


Contribuir
----------
1. Haz un fork del repositorio
2. Crea una rama: git checkout -b feature/nuevo-plugin
3. Realiza cambios: git commit -m "Añadido plugin X"
4. Sube los cambios: git push origin feature/nuevo-plugin
5. Abre un Pull Request


Licencia
--------
MIT License. Consulta el archivo LICENSE para mas detalles.


Contacto
--------
GitHub:  https://github.com/TU_USUARIO
Issues:  https://github.com/TU_USUARIO/rust-plugins-devblog133/issues
Email:   tu@email.com


Agradecimientos
---------------
- Facepunch Studios por crear Rust
- Oxide/uMod Team por el framework de plugins
- Comunidad de Rust por reportar exploits y sugerir mejoras
