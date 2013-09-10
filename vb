#!/bin/bash
# 2013-09-10: Derek Gottlieb

function print_usage {
 echo "Usage: $0 vmname [status|start|startrdp|reboot|poweroff|savestate|resume|ssh|sshport|rdpport] [user]"
}

if [[ -z "$1" ]] || [[ -z "$2" ]]; then
 print_usage
 exit
fi

VMNAME=$1
ACTION=$2
  
if [[ ! -z "$3" ]]; then
 GUESTUSER=$3
else
 GUESTUSER=$USER
fi

VM_EXISTS=$(vboxmanage list vms | grep -c ^\"$VMNAME\")
if [ "$VM_EXISTS" -eq 0 ]; then
 printf "\nERROR: $1 is not a valid VM name!\n\n"
 print_usage
 exit
fi

if [[ $ACTION == "status" ]]; then
 # print out the machine state
 vboxmanage showvminfo $VMNAME | grep State
elif [[ $ACTION == "start" ]]; then
 # start without vrdp
 #vboxheadless -s $VMNAME --vrdp=off &
 # start with vrdp based on config
 vboxheadless -s $VMNAME --vrdp=config &
elif [[ $ACTION == "startrdp" ]]; then
 # start with vrdp
 vboxheadless -s $VMNAME --vrdp=on &
elif [[ $ACTION == "reboot" ]]; then
 # send ctrl+alt+del to guest machine
 vboxmanage $VMNAME keyboardputscancode 1d 38 53 b8 9d
elif [[ $ACTION == "poweroff" ]]; then
 # simulate pressing power off button, will power cut immediately
 vboxmanage controlvm $VMNAME poweroff
elif [[ $ACTION == "savestate" ]]; then
 vboxmanage controlvm $VMNAME savestate
elif [[ $ACTION == "resume" ]]; then
 vboxmanage controlvm $VMNAME resume
elif [[ $ACTION == "ssh" ]]; then
 IS_GUESTSSH=$(vboxmanage showvminfo $VMNAME | grep guest | grep -c ssh)
 if [ "$IS_GUESTSSH" -gt 0 ]; then
  PORT=$(vboxmanage showvminfo $VMNAME | grep guest | grep ssh | awk 'BEGIN {FS=","} {print $4}' | awk '{print $NF}')
  echo "$VMNAME: ssh -p $PORT $GUESTUSER@localhost"
  ssh -p $PORT $GUESTUSER@localhost
 else 
  echo "$VMNAME: no guest ssh forward rule"
 fi
elif [[ $ACTION == "sshport" ]]; then
 IS_GUESTSSH=$(vboxmanage showvminfo $VMNAME | grep guest | grep -c ssh)
 if [ "$IS_GUESTSSH" -gt 0 ]; then
  PORT=$(vboxmanage showvminfo $VMNAME | grep guest | grep ssh | awk 'BEGIN {FS=","} {print $4}' | awk '{print $NF}')
  echo "$VMNAME: ssh -p $PORT $GUESTUSER@localhost"
 else 
  echo "$VMNAME: no guest ssh forward rule"
 fi
elif [[ $ACTION == "rdpport" ]]; then
 IS_RDP_PORT=$(vboxmanage showvminfo $VMNAME | grep -c "VRDE property: TCP/Ports")
 if [ "$IS_RDP_PORT" -gt 0 ]; then
  vboxmanage showvminfo $VMNAME | grep "VRDE property: TCP/Ports"
 else 
  echo "$VMNAME: no RDP port configured"
 fi
else
 printf "\nERROR: $2 is not a supported action!\n\n"
 print_usage
 exit
fi
