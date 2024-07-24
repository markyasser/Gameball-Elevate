pipeline {
    agent any

    environment {
        DOTNET_ROOT = tool name: 'dotnet', type: 'DotNetCoreSdkInstaller'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        stage('Restore') {
            steps {
                script {
                    def dotnetHome = tool name: 'dotnet', type: 'DotNetCoreSdkInstaller'
                    env.PATH = "${dotnetHome}/bin:${env.PATH}"
                }
                sh 'dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }
    }

    post {
        success {
            echo 'Build succeeded!'
        }
        failure {
            echo 'Build failed!'
        }
    }
}
