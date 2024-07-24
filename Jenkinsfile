pipeline {
    agent any

    environment {
        DOTNET_ROOT = tool name: 'dotnet', type: 'CustomTool'
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
                    env.PATH = "${env.DOTNET_ROOT}:${env.PATH}"
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
