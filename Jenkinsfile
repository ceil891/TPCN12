pipeline {
    agent any

    stages {
        stage('Clone') {
            steps {
                echo 'Cloning source code'
                git branch: 'main', url: 'https://github.com/ceil891/TPCN12.git'
            }
        }

        stage('restore package') {
		steps
		{
			echo 'Restore package'
			bat 'dotnet restore'
		}
	     }


        stage ('build') {
		steps {
			echo 'build project netcore'
			bat 'dotnet build  --configuration Release'
		}
	}


        stage('Tests') {
            steps {
                echo 'Running tests...'
                bat 'dotnet test --no-build --verbosity normal'
            }
        }

        stage('Publish to ./publish folder') {
            steps {
                echo 'Publishing to ./publish folder...'
                bat 'dotnet publish -c Release -o ./publish'
            }
        }
        stage('Check .NET SDK') {
    steps {
        bat 'dotnet --version'
        bat 'dotnet --list-sdks'
    }
}

       stage('Copy to IIS folder') {
    steps {
        echo 'Copying to IIS root folder...'
        bat 'iisreset /stop'
        bat '''
        if exist "%WORKSPACE%\\publish" (
            xcopy "%WORKSPACE%\\publish" "C:\\inetpub\\wwwroot\\TrienKhai" /E /Y /I /R
        ) else (
            echo "Publish folder not found!"
            exit /b 1
        )
        '''
        bat 'iisreset /start'
    }
}


        stage('Deploy to IIS') {
            steps {
                echo 'Creating IIS Website if not exists...'
                powershell '''
                    Import-Module WebAdministration
                    if (-not (Test-Path IIS:\\Sites\\TKPM)) {
                        New-Website -Name "TKPM4" -Port 85 -PhysicalPath "C:\\inetpub\\wwwroot\\TrienKhai" -Force
                    }
                '''
            }
        }
    }
}
