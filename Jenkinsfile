pipeline {
    agent any

    environment {
        // Using .NET 7.0 SDK
        DOTNET_CORE_SDK_VERSION = '7.0'
        DOCKER_IMAGE = 'nitefallz/mystuff' // Adjust with your Docker Hub repo name
        DOCKER_TAG = 'latest'
        DOCKER_CREDENTIALS_ID = 'docker-hub-credentials' // Use the actual ID for Docker Hub credentials
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm: [$class: 'GitSCM', userRemoteConfigs: [[url: 'https://github.com/nitefallz/PhoenixSagas']]]
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build PhoenixSagas/PhoenixSagas.TCPServer/PhoenixSagas.TCPServer.csproj --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'No tests specified. Add commands to run tests here.'
                // Ideally, include steps to run your project's tests
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    // Correctly change to the PhoenixSagas directory before building the Docker image
                    dir('PhoenixSagas/') { 
                        sh 'docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} .'
                    }
                }
            }
        }

        stage('Push Docker Image') {
            steps {
                script {
                    withCredentials([usernamePassword(credentialsId: "${DOCKER_CREDENTIALS_ID}", usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                        sh 'echo $DOCKER_PASSWORD | docker login --username $DOCKER_USERNAME --password-stdin'
                        sh 'docker push ${DOCKER_IMAGE}:${DOCKER_TAG}'
                    }
                }
            }
        }

		stage('Deploy') {
		    steps {
		        script {
		            // Stopping and removing the old container if it exists
		            sh """
		            docker stop phoenixsagas-tcpserver-container || true
		            docker rm phoenixsagas-tcpserver-container || true
		            """
		            // Running the new container
		            sh """
		            docker run -d --name phoenixsagas-tcpserver-container -p 4000:4000 ${DOCKER_IMAGE}:${DOCKER_TAG}
		            """
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
