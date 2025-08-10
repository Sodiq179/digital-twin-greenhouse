pipeline{
    agent any
    stages {
        stage('Installing new update and updating the node packages...'){
            steps {
                sh '''
                pwd
                sudo apt-get update && sudo apt-get upgrade --yes
                sudo cp /var/lib/jenkins/workspace/config.env /var/lib/jenkins/workspace/ClimdesBackend/
                npm install
                pm2 restart all
                '''
            }
        }

    }
}