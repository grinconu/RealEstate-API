# 🏠 Real Estate API

Este proyecto es una **API para gestión de propiedades inmobiliarias (Real Estate)** desarrollada en **.NET 9**, siguiendo los principios de **Arquitectura Limpia**.

---

## ✅ Requisitos

Para abrir y ejecutar este proyecto necesitas:

- **Windows**: [Visual Studio 2022+](https://visualstudio.microsoft.com/) con soporte para .NET 9  
- **macOS**: [JetBrains Rider](https://www.jetbrains.com/rider/) o [Visual Studio Code](https://code.visualstudio.com/) configurado para **.NET 9**  
- **Base de datos**: SQL Server (local o en la nube)  
- Configurar el **Connection String** y las opciones de autenticación mediante **User Secrets**

---

## 🏗️ Arquitectura del Proyecto

El proyecto está organizado con **Arquitectura Limpia** y los siguientes proyectos:

RealEstate_API

── API

   └── RealEstate.Api             → Capa de presentación (controladores / endpoints)
   
── Core

   ├── RealEstate.Application     → Casos de uso (Use Cases), lógica de negocio
   
   └── RealEstate.Domain          → Entidades del dominio
   
── Infrastructure

   └── RealEstate.Infrastructure  → Persistencia, servicios externos, implementación de repositorios
   
── Shared

   └── RealEstate.Shared          → Utilidades y componentes compartidos
   
── Test

   └── RealEstate.Test            → Pruebas unitarias de la capa Application (Use Cases)


---

## 🧪 Pruebas

Se crearon pruebas unitarias para la **lógica de negocio (Use Cases)**, ubicadas en: Test/RealEstate.Test

Estas pruebas cubren la validación de reglas en los casos de uso principales.

---

## 🔐 Seguridad

El proyecto implementa **seguridad con JWT (JSON Web Tokens)**:

- Todos los servicios protegidos requieren un token válido.  
- Se creó un servicio de autenticación para **generar tokens JWT**, que luego pueden usarse en los headers: Authorization: Bearer {token} al consumir el resto de endpoints.  

---


## 📘 Documentación con OpenAPI

Este proyecto utiliza **OpenAPI** para documentar automáticamente los endpoints disponibles en la API.  

- Al iniciar la aplicación, se levanta en el navegador una página con el **JSON de especificación OpenAPI**.  
- Ese archivo puede ser importado fácilmente en **Postman**, **Insomnia** u otros programas de prueba de APIs para consumir los servicios directamente.

---

## ⚙️ Configuración con User Secrets

Este proyecto utiliza **User Secrets** para configurar la conexión a la base de datos y los parámetros de autenticación, evitando que información sensible quede en el código fuente.

Configura los secretos ejecutando en la raíz del proyecto:

```bash
dotnet user-secrets init

Luego agrega los valores:
dotnet user-secrets set "ConnectionStrings:DBConnection" "Server=localhost;Database=RealEstateDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"

dotnet user-secrets set "AuthenticationConfig:Issuer" "https://realestate-api"
dotnet user-secrets set "AuthenticationConfig:Audience" "realestate-test"
dotnet user-secrets set "AuthenticationConfig:HashSecret" "yourHashSecret"
dotnet user-secrets set "AuthenticationConfig:JwtSecret" "yourJwtSecret"
dotnet user-secrets set "AuthenticationConfig:JwtTokenExpirationInMin" "15"
dotnet user-secrets set "AuthenticationConfig:TokenAuth" "yourTokenAuthKey"
```

Ejemplo de configuración en secrets.json
```
{
  "ConnectionStrings": {
    "DBConnection": "Server=localhost;Database=RealEstateDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  },
  "AuthenticationConfig": {
    "Issuer": "https://realestate-api",
    "Audience": "realestate-test",
    "HashSecret": "yourHashSecret",
    "JwtSecret": "yourJwtSecret",
    "JwtTokenExpirationInMin": "15",
    "TokenAuth": "yourTokenAuthKey"
  }
}
```

## 🗄️ Restaurar la base de datos desde `RealEstateTest.bacpac`

Este proyecto incluye un archivo de respaldo (`RealEstateTest.bacpac.zip`) en la carpeta `backup-db` situada en la raíz del repositorio.
Antes de poder restaurar la base, **es necesario descomprimir el archivo** para obtener el fichero `.bacpac`: RealEstateTest.bacpac

Los pasos a continuación te muestran cómo importar este archivo con `SqlPackage` para restaurar la base de datos en tu entorno local o contenedor.

---

### ✅ Requisitos previos

- Tener instalado **SqlPackage** (disponible para Windows, macOS y Linux)  
- Tener acceso al servidor SQL al que vas a importar (por ejemplo, `localhost`, instancia en contenedor, etc.), con credenciales válidas y permisos para crear bases de datos  
- Que no exista ya una base de datos con el nombre destino, o que esté vacía (el import no sobrescribe una base con objetos existentes)

---

### 🔧 Comando de importación con SqlPackage

Ejecuta el siguiente comando desde la raíz del proyecto (o desde la carpeta donde está `backup-db`):

```bash
SqlPackage /Action:Import \
  /SourceFile:"./backup-db/RealEstateTest.bacpac" \
  /TargetServerName:"localhost" \
  /TargetDatabaseName:"RealEstateDb" \
  /TargetUser:"sa" \
  /TargetPassword:"YourStrongPassword" \
  /p:DatabaseEdition="Developer" \
  /p:DatabaseServiceObjective="S0"
