# CambioEstatus

Aplicación de escritorio (Windows Forms) para conectar al SAP Business One Service Layer y automatizar el cambio del campo `U_Clasificacion_venta` en artículos seleccionados.

## Características

- Login/Logout contra SAP B1 Service Layer.
- Consulta OData (crossjoin) para localizar artículos con `U_Clasificacion_venta = 'NOVEDAD'` y stock en almacén `1`.
- Actualiza (PATCH) el campo `U_Clasificacion_venta` a `INNOVACION` para los artículos listados.
- Almacena la configuración local cifrada con DPAPI en `%APPDATA%\CambioEstatus\config.dat`.
- Interfaz con vista de resultados y barra de progreso por actualización.

## Requisitos

- Visual Studio 2022 o posterior.
- .NET 8 SDK.
- Acceso al SAP Business One Service Layer con credenciales y permisos para PATCH sobre `Items`.

## Dependencias

- `Newtonsoft.Json` (NuGet)
- System.Net.Http (incluido en .NET)

## Instalación y ejecución

1. Clonar el repositorio:
   git clone https://github.com/LupilloJMX/CambioEstatus-sap-b1.git

2. Abrir la solución en Visual Studio 2022.
3. Restaurar paquetes NuGet.
4. Compilar la solución (Target: .NET 8).
5. Ejecutar la aplicación.

## Configuración

- Desde el botón `Configuración` introducir:
  - URL del Service Layer (ej.: `https://miServidor:50000/b1s/v1/`)
  - Usuario
  - Contraseña (no se muestra en la UI)
  - CompanyDB

- La configuración se guarda cifrada por usuario en `%APPDATA%\CambioEstatus\config.dat`.

## Uso rápido

1. Presionar `Conectar` para autenticarse.
2. Presionar `Buscar` para listar artículos que cumplen los criterios.
3. Revisar la lista y presionar `Actualizar` para cambiar `U_Clasificacion_venta` a `INNOVACION`.
4. Presionar `Desconectar` para cerrar sesión.

## Notas de seguridad y advertencias

- La aplicación actualmente ignora la validación del certificado SSL (`ServerCertificateCustomValidationCallback` acepta cualquier certificado). Esto facilita pruebas con certificados autofirmados, pero es un riesgo en producción. Recomendado: usar certificados válidos y eliminar la aceptación automática.
- La configuración se cifra con DPAPI ligada al usuario local (DataProtectionScope.CurrentUser). Si se necesita uso multiusuario o centralizado, considerar alternativa segura.
- Asegurarse de que el usuario de Service Layer tenga permisos para modificar `Items`.

## Detalles técnicos

- `HttpClient` se inicializa con `BaseAddress` al Service Layer y se gestionan cookies `B1SESSION` y `ROUTEID`.
- Consulta principal (ejemplo): crossjoin entre `Items` y `Items/ItemWarehouseInfoCollection` con filtros `U_Clasificacion_venta eq 'NOVEDAD'`, `WarehouseCode eq '1'`, `InStock ge 1`.
- Para cada artículo se envía un `PATCH` a `Items('{ItemCode}')` con `{ "U_Clasificacion_venta": "INNOVACION" }`.

## Contribuir

- Abrir issues o pull requests en el repositorio.
- Mantener pruebas y documentar cambios.
- Seguir las convenciones del proyecto (añadir .editorconfig / CONTRIBUTING.md si procede).

## Licencia / Contacto

Este repositorio se publica sin licencia (no hay archivo `LICENSE`).  
Contacto: Ing. Jiménez — ing.jimenez@live.com
