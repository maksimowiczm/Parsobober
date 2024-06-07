### Run with DockerHub
```bash
curl https://raw.githubusercontent.com/BobryPb/Parsobober/main/tests/SPA-Official/cez/dropbox
docker run -i -v ./parsobober-code:/app/code maksimowiczm/parsobober
```

### Run with Docker build
```bash
git clone https://github.com/BobryPb/Parsobober parsobober
cd parsobober
docker build . -t parsobober
docker run -i -v ./tests/SPA-Official/cez/dropbox:/app/code parsobober
```

### Run with dotnet
```bash
git clone https://github.com/BobryPb/Parsobober parsobober
cd parsobober
dotnet run -c release --project src/Parsobober.Cli "./tests/SPA-Official/cez/dropbox"
```
