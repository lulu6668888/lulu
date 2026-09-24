pipeline {

    agent any

    parameters {

        choice(
            name: 'PLATFORM',
            choices: ['Android', 'iOS'],
            description: 'Build Platform'
        )

        string(
            name: 'APP_VERSION',
            defaultValue: '1.0.0',
            description: 'Application Version'
        )

        string(
            name: 'APP_BUILD_CODE',
            defaultValue: '1',
            description: 'Android versionCode / iOS buildNumber'
        )
    }

    environment {

        UNITY =
            '/Applications/Unity/Hub/Editor/6000.3.23f1/Unity.app/Contents/MacOS/Unity'
    }

    stages {

        stage('Environment') {

            steps {

                sh '''
                    echo "===== Environment ====="

                    whoami
                    pwd

                    echo "===== Git ====="
                    which git
                    git --version

                    echo "===== Java ====="
                    which java || true
                    java -version || true

                    echo "===== Unity ====="
                    "${UNITY}" -version
                '''
            }
        }

        stage('Build Unity') {

            steps {

                sh '''
                    rm -rf Build
                    mkdir -p Build

                    "${UNITY}" \
                    -projectPath "${WORKSPACE}" \
                    -batchmode \
                    -quit \
                    -buildTarget "${PLATFORM}" \
                    -executeMethod JenkinsBuild.Build \
                    -platform "${PLATFORM}" \
                    -appVersion "${APP_VERSION}" \
                    -buildNumber "${APP_BUILD_CODE}" \
                    -logFile "${WORKSPACE}/Build/unity.log"
                '''
            }
        }

        stage('Archive') {

            steps {

                archiveArtifacts(
                    artifacts: 'Build/**/*',
                    fingerprint: true
                )
            }
        }
    }
}