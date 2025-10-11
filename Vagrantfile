Vagrant.configure("2") do |config|
  config.vm.box = "ubuntu/jammy64"
  config.vm.boot_timeout = 600
  config.vm.hostname = "ubuntu-vm"

  config.vm.provider "virtualbox" do |vb|
    vb.memory = 2048
    vb.cpus = 2
  end

  config.vm.provision "shell", inline: <<-SHELL
    echo "=== Початок налаштування ==="
    sudo apt update
    sudo apt install -y dotnet-sdk-8.0

    echo "=== Імітація приватного NuGet репозиторію ==="
    dotnet nuget add source "https://baget.example.com/v3/index.json" --name PrivateRepo

    echo "=== Створення тестового застосунку ==="
    dotnet new console -n TestApp
    cd TestApp

    echo "=== Встановлення пакету AirQualityMonitoring ==="
    dotnet add package AirQualityMonitoring --version 1.0.0 --source PrivateRepo

    echo "=== Запуск застосунку ==="
    dotnet run
  SHELL
end