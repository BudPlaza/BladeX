-- Powered by the BudPlaza project

fx_version 'cerulean'
games { 'gta5' }

author 'WithLithum <RelaperCrystal@163.com>'
description 'BladeX'
version '0.1.1'

-- To make sure the UI system work
-- Load LemonUI as script

client_scripts {
	"BladeXClient.net.dll",
	"LemonUI.FiveM.net.dll"
}

-- Server side dependencies will just work however

server_script "BladeX.net.dll"

dependency "chat"