# ASP.NET Integration Testing
## Building docker images
### The application
```sh
docker build . -t app
```
### Component tests
```sh
docker build -f Dockerfile.ct . -t component-tests
```
### Integration tests
```sh
docker build -f Dockerfile.it . -t integration-tests
```
