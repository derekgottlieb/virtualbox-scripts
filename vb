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
  
VM_EXISTS=$(vboxmanage list vms | grep -c ^\"$VMNAME\")
if [ "$VM_EXISTS" -eq 0 ]; then
 printf "\nERROR: $1 is not a valid VM name!\n\n"
 print_usage
 exit
fi

if [ "$ACTION" == "ssh" ] || [ "$ACTION" == "sshport" ]; then
 # Determine if we have a port forward set for ssh
 IS_GUESTSSH=$(vboxmanage showvminfo $VMNAME | grep guest | grep -c ssh)
 if [ "$IS_GUESTSSH" -gt 0 ]; then
  PORT=$(vboxmanage showvminfo $VMNAME | grep guest | grep ssh | awk 'BEGIN {FS=","} {print $4}' | awk '{print $NF}')
 else 
  echo "$VMNAME: no guest ssh forward rule"
  exit
 fi
 
 # Only care about the guest user id for ssh-related actions
 if [[ ! -z "$3" ]]; then
  GUESTUSER=$3
 else
  GUESTUSER=$USER
 fi
fi

case $ACTION in 
 "status")
  # print out the machine state
  vboxmanage showvminfo $VMNAME | grep State
  ;;
 "start")
  # start without vrdp
  #vboxheadless -s $VMNAME --vrdp=off &
  # start with vrdp based on config
  vboxheadless -s $VMNAME --vrdp=config &
  ;;
 "startrdp")
  # start with vrdp
  vboxheadless -s $VMNAME --vrdp=on &
  ;;
 "reboot")
  # send ctrl+alt+del to guest machine
  vboxmanage $VMNAME keyboardputscancode 1d 38 53 b8 9d
  ;;
 "poweroff")
  # simulate pressing power off button, will power cut immediately
  vboxmanage controlvm $VMNAME poweroff
  ;;
 "savestate")
  vboxmanage controlvm $VMNAME savestate
  ;;
 "resume")
  vboxmanage controlvm $VMNAME resume
  ;;
 "ssh")
   echo "$VMNAME: ssh -p $PORT $GUESTUSER@localhost"
   ssh -p $PORT $GUESTUSER@localhost
  ;;
 "sshport")
   echo "$VMNAME: ssh -p $PORT $GUESTUSER@localhost"
  ;;
 "rdpport")
  IS_RDP_PORT=$(vboxmanage showvminfo $VMNAME | grep -c "VRDE property: TCP/Ports")
  if [ "$IS_RDP_PORT" -gt 0 ]; then
   vboxmanage showvminfo $VMNAME | grep "VRDE property: TCP/Ports"
  else 
   echo "$VMNAME: no RDP port configured"
  fi
  ;;
 *)
  printf "\nERROR: $2 is not a supported action!\n\n"
  print_usage
  exit
 ;;
esac
