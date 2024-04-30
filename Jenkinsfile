pipeline {
    agent any

   environment {
    // Using .NET 7.0 SDK
    DOTNET_CORE_SDK_VERSION = '7.0'
    TCP_IMAGE = 'nitefallz/phoenix-tcpserver'
    TCP_TAG = "build-${BUILD_NUMBER}"
    GAME_IMAGE = 'nitefallz/phoenix-gameserver'
    GAME_TAG = "build-${BUILD_NUMBER}"
    DOCKER_CREDENTIALS_ID = 'docker-hub-credentials' // Use the actual ID for Docker Hub credentials
    NUGET_SERVER_URL = 'https://gomezdev.hopto.org:8090/NugetServer/nuget' // Your NuGet server URL
    NUGET_API_KEY = '711cf1f2-d71d-4a75-9293-ecc6e2992228' // Your NuGet server API Key
}


    stages {
        stage('Checkout') {
            steps {
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }
        
        stage('Restore NuGet Packages') {
            steps {
                // Use the newly added source along with the default nuget.org source
                sh 'dotnet restore PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/Models/PhoenixSagas.Models.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/GameEngine/PhoenixSagas.GameEngine.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/TCPServer/PhoenixSagas.TCPServer.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
                sh 'dotnet restore PhoenixSagas/GameServer/PhoenixSagas.GameServer.csproj --source "${NUGET_SERVER_URL}" --source "https://api.nuget.org/v3/index.json"'
            }
        }

        stage('Build Projects') {
            steps {
                sh 'dotnet build PhoenixSagas/Models/PhoenixSagas.Models.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/GameEngine/PhoenixSagas.GameEngine.csproj --configuration Release'
                sh 'dotnet build PhoenixSagas/GameServer/PhoenixSagas.GameServer.csproj --configuration Release'
            }
        }


        stage('Test') {
            steps {
                echo 'No tests specified. Add commands to run tests here.'
                // Ideally, include steps to run your project's tests
            }
        }

        stage('Package and Push NuGet') {
            steps {
                script {
                    // Package the .NET project
                    sh 'dotnet pack PhoenixSagas/Kafka/PhoenixSagas.Kafka.csproj --configuration Release -o ./nupkgs'
                    sh 'dotnet pack PhoenixSagas/Models/PhoenixSagas.Models.csproj --configuration Release -o ./nupkgs'
                    
                    // Find and push each package to the NuGet server
                    def nugetPackages = findFiles(glob: 'nupkgs/*.nupkg')
                    nugetPackages.each {
                        sh "dotnet nuget push '${it.path}' --source ${NUGET_SERVER_URL} --api-key ${NUGET_API_KEY} --skip-duplicate"
                    }
                }
            }
        }

        
       stage('Build Docker Images') {
    steps {
        script {
            dir('PhoenixSagas/') { 
                sh "docker build -f Dockerfile.TcpServer --no-cache --build-arg NUGET_SOURCE=\${NUGET_SERVER_URL} -t \${TCP_IMAGE}:\${TCP_TAG} ."
            }
            dir('PhoenixSagas/') { 
                sh "docker build -f Dockerfile.GameServer --no-cache --build-arg NUGET_SOURCE=\${NUGET_SERVER_URL} -t \${GAME_IMAGE}:\${GAME_TAG} ."
            }
        }
    }
}

stage('Push Docker Image') {
    steps {
        script {
            withCredentials([usernamePassword(credentialsId: "${DOCKER_CREDENTIALS_ID}", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                        sh 'echo $DOCKER_PASSWORD | docker login --username $DOCKER_USERNAME --password-stdin'
                        sh 'docker push ${TCP_IMAGE}:${TCP_TAG}'
                        sh 'docker push ${GAME_IMAGE}:${GAME_TAG}'
            }
        }
    }
}


   stage('Deploy') {
    steps {
        script {
            // Pull the latest Docker image and redeploy the container
            sh 'docker pull \${TCP_IMAGE}:\${TCP_TAG}'
            sh 'docker stop phoenixsagas-tcpserver-container || true'
            sh 'docker rm phoenixsagas-tcpserver-container || true'
            sh 'docker run -d --name phoenixsagas-tcpserver-container -p 4000:4000 \${TCP_IMAGE}:\${TCP_TAG}'
        }
    }
}

stage('Deploy GameEngine') {
    steps {
        script {
            // Pull the latest Docker image and redeploy the GameEngine container
            sh 'docker pull \${GAME_IMAGE}:\${GAME_TAG}'
            sh 'docker stop gameengine-container || true'
            sh 'docker rm gameengine-container || true'
            sh 'docker run -d --name gameengine-container -p 8000:8000 \${GAME_IMAGE}:\${GAME_TAG}'
        }
    }
}

    }

    post {
        success {
            echo 'Build, Docker image push, and deployment stages completed successfully.'
        }
        failure {
            echo 'An error occurred during the build.'
        }
    }
}
