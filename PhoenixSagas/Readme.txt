# Build the TCPServer image
docker build -t nitefallz/tcpserversaga -f Dockerfile.TcpServer .

# Build the GameServer image
docker build -t nitefallz/gameserversaga -f Dockerfile.GameServer .

dotnet pack PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release -o ./nupkgs
dotnet nuget push nupkgs/PhoenixSagas.Kafka.1.0.4.nupkg --source https://gomezdev.hopto.org:8090/NugetServer/nuget --api-key 711cf1f2-d71d-4a75-9293-ecc6e2992228 --skip-duplicate

docker tag nitefallz/phoenix-gameserver:latest nitefallz/phoenix-gameserver:v1.0
docker push nitefallz/phoenix-gameserver:v1.0