#!/bin/bash

#OLD METHOD UNRELIABLE
#mkdir /tmp/StreamRamDisk
#sudo chmod 777 /tmp/StreamRamDisk
#sudo mount -t tmpfs -o size=128m StreamRamDisk /tmp/StreamRamDisk

#NEW METHOD PERSISTS ON REBOOT
start_path=$(pwd -P)
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
cd "$start_path"
script_path="$parent_path/systemd/system/home-pi-StreamRamDisk.mount"
sudo cp "$script_path" /etc/systemd/system/
sudo systemctl enable home-pi-StreamRamDisk.mount
