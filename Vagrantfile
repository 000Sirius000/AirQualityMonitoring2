Vagrant.configure("2") do |config|

  # -------------------------------
  # 🟢 1) Ubuntu (Linux, .NET SDK 8)
  # -------------------------------
  config.vm.define "ubuntu" do |ubuntu|
    ubuntu.vm.box = "ubuntu/jammy64"
    ubuntu.vm.boot_timeout = 600
    ubuntu.vm.hostname = "ubuntu-vm"

    ubuntu.vm.provider "virtualbox" do |vb|
      vb.memory = 2048
      vb.cpus = 2
    end

    ubuntu.vm.provision "shell", inline: <<-SHELL
      echo "=== Початок налаштування Ubuntu ==="
      sudo apt update
      sudo apt install -y dotnet-sdk-8.0

      echo "=== Додаю локальний NuGet-репозиторій ==="
      dotnet nuget remove source LocalRepo || true
      dotnet nuget add source "/vagrant/bin/Release" --name LocalRepo

      echo "=== Створюю тестовий застосунок ==="
      rm -rf ~/TestApp
      dotnet new console -n TestApp
      cd TestApp

      echo "=== Встановлення пакету AirQualityMonitoring ==="
      dotnet add package AirQualityMonitoring --version 1.0.0 --source /vagrant/bin/Release

      echo "=== Запуск застосунку (Ubuntu) ==="
      dotnet run
    SHELL
  end

  # -------------------------------
  # 🔵 2) Debian 12 (Bookworm)
  # -------------------------------
  config.vm.define "debian" do |debian|
    debian.vm.box = "debian/bookworm64"
    debian.vm.hostname = "debian-vm"
    debian.vm.boot_timeout = 900

    debian.vm.provider "virtualbox" do |vb|
      vb.memory = 2048
      vb.cpus = 2
    end

    debian.vm.provision "shell", inline: <<-SHELL
      echo "=== Початок налаштування Debian ==="
      sudo apt update
      sudo apt install -y wget apt-transport-https software-properties-common

      echo "=== Додаю офіційний репозиторій Microsoft ==="
      wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
      sudo dpkg -i packages-microsoft-prod.deb
      rm packages-microsoft-prod.deb

      sudo apt update
      sudo apt install -y dotnet-sdk-8.0

      echo "=== Додаю локальний NuGet-репозиторій ==="
      dotnet nuget remove source LocalRepo || true
      dotnet nuget add source "/vagrant/bin/Release" --name LocalRepo

      echo "=== Створюю тестовий застосунок ==="
      rm -rf ~/TestApp
      dotnet new console -n TestApp
      cd TestApp

      echo "=== Встановлення пакету AirQualityMonitoring ==="
      dotnet add package AirQualityMonitoring --version 1.0.0 --source /vagrant/bin/Release

      echo "=== Запуск застосунку (Debian) ==="
      dotnet run
    SHELL
  end


end