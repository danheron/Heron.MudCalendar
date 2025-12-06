## Install .Net SDKs (6.0, 7.0, 8.0, 9.0, 10.0)
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update && \
    export DEBIAN_FRONTEND=noninteractive && \
    sudo apt-get install -y dotnet-sdk-8.0 dotnet-sdk-9.0 dotnet-sdk-10.0

## Configure git
git config --global core.autocrlf input

## Enable local HTTPS for .NET
dotnet dev-certs https --trust

# Set vscode user as owner of workspaces folder
sudo chown vscode /workspaces

# Clone and checkout MudBlazor mudcalendar branch and restore
git clone https://github.com/danheron/MudBlazor /workspaces/MudBlazor
cd /workspaces/MudBlazor
git checkout mudcalendar
cd src
dotnet restore

## Restore MudCalendar
cd /workspaces/Heron.MudCalendar
dotnet restore Heron.MudCalendarWithDocs.sln

# Build debug and release versions of MudCalendar
dotnet build /workspaces/Heron.MudCalendar/Heron.MudCalendar -c Debug --no-restore
dotnet build /workspaces/Heron.MudCalendar/Heron.MudCalendar -c Release --no-restore

# Build MudBlazor docs
dotnet build /workspaces/MudBlazor/src/MudBlazor.Docs -c Release --no-restore

# Build MudCalendar projects
dotnet build /workspaces/Heron.MudCalendar/Heron.MudCalendarWithDocs.sln

## AZURE CLI EXTENSIONS ##
# Uncomment the below to install Azure CLI extensions
# extensions=(account alias deploy-to-azure functionapp subscription webapp)
# for extension in $extensions;
# do
#     az extension add --name $extension
# done

## AZURE BICEP CLI ##
# Uncomment the below to install Azure Bicep CLI.
# az bicep install

## AZURE FUNCTIONS CORE TOOLS ##
# Uncomment the below to install Azure Functions Core Tools. Make sure you have installed node.js
# npm i -g azure-functions-core-tools@4 --unsafe-perm true

## Azurite ##
# Uncomment the below to install Azurite. Make sure you have installed node.js
# npm install -g azurite

## AZURE STATIC WEB APPS CLI ##
# Uncomment the below to install Azure Static Web Apps CLI. Make sure you have installed node.js
# npm install -g @azure/static-web-apps-cli

## AZURE DEV CLI ##
# Uncomment the below to install Azure Dev CLI. Make sure you have installed Azure CLI and GitHub CLI
# curl -fsSL https://aka.ms/install-azd.sh | bash

## OH-MY-ZSH PLUGINS & THEMES (POWERLEVEL10K) ##
# Uncomment the below to install oh-my-zsh plugins and themes (powerlevel10k) without dotfiles integration
#git clone https://github.com/zsh-users/zsh-completions.git $HOME/.oh-my-zsh/custom/plugins/zsh-completions
#git clone https://github.com/zsh-users/zsh-syntax-highlighting.git $HOME/.oh-my-zsh/custom/plugins/zsh-syntax-highlighting
#git clone https://github.com/zsh-users/zsh-autosuggestions.git $HOME/.oh-my-zsh/custom/plugins/zsh-autosuggestions

# git clone https://github.com/romkatv/powerlevel10k.git $HOME/.oh-my-zsh/custom/themes/powerlevel10k --depth=1
#ln -s $HOME/.oh-my-zsh/custom/themes/powerlevel10k/powerlevel10k.zsh-theme $HOME/.oh-my-zsh/custom/themes/powerlevel10k.zsh-theme

## OH-MY-POSH ##
# Uncomment the below to install oh-my-posh
# sudo wget https://github.com/JanDeDobbeleer/oh-my-posh/releases/latest/download/posh-linux-amd64 -O /usr/local/bin/oh-my-posh
# sudo chmod +x /usr/local/bin/oh-my-posh