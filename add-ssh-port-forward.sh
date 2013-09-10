#!/bin/bash
# 2013-09-10: Derek Gottlieb

if [[ -z "$1" ]]; then
 echo "Usage: $0 vmname"
 exit
fi

if [[ -z "$2" ]]; then
 NIC=1
else
 NIC=$2
fi

VMNAME=$1

if [ ! -e $HOME/.next-vb-ssh-port ]; then
 echo "2222" > $HOME/.next-vb-ssh-port
fi

PORT=$(cat $HOME/.next-vb-ssh-port)
vboxmanage modifyvm ${VMNAME} --natpf${NIC} "guestssh,tcp,127.0.0.1,${PORT},,22"
RETURN=$?

if [ "$RETURN" -ne "0" ]; then
 echo "ERROR: failed to add port forward for ssh to ${VMNAME}"
else
 echo "Using port $((PORT++))"
 echo $PORT > $HOME/.next-vb-ssh-port
 echo "Setting next available port forward to ${PORT}"
fi
