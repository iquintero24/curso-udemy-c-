# versionamineto de una API

permite mantener multiples versiones de una API REST para no romper integraciones existentres al hacer cambios o mejoras.

#### Es esencial cuando:

- Se cambia de esturcturas de respuestas o contratos 
- Se elimina endpoints.
- Se introducen nuevas funcionalidades incompatibles 

### Enfoques de versionamiento de un api en .NET

- en la ruta (/api/v1/products)
- En el query string (?api-version=1.0)
- En el header (api-version:1.0)

### Extensiones:

- Asp.Version.Mvc 
- Asp.Version.Mvc.ApiExplorer 


