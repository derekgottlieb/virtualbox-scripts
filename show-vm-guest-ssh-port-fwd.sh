#!/bin/bash
# 2013-09-10: Derek Gottlieb

if [[ -z "$1" ]]; then
 echo "Usage: $0 vmname"
 exit
fi

VMNAME=$1

IS_GUESTSSH=$(vboxmanage showvminfo $VMNAME | grep guestlocalssh | wc -l)
if [ "$IS_GUESTSSH" -gt 0 ]; then
 PORT=$(vboxmanage showvminfo $VMNAME | grep guestlocalssh | awk 'BEGIN {FS=","} {print $4}'  | awk '{print $NF}')
 echo "$VMNAME: ssh -p $PORT userid@localhost"
else 
 echo "$VMNAME: no guest ssh forward rule"
fi
