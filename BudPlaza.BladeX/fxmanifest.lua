fx_version 'cerulean'
games { 'gta5' }

author 'WithLithum <RelaperCrystal@163.com>'
description 'CharaManager and Client'
version '1.0.0'

client_scripts {
	"LithiumDev.CharaClient.net.dll",
	"LemonUI.FiveM.net.dll"
}

server_script "LithiumDev.CharaManager.net.dll"

dependency "chat"