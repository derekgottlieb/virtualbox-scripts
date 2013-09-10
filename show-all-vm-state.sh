#!/bin/bash
# 2013-09-10: Derek Gottlieb

SCRIPTPATH=$(dirname $(readlink -f $0))

for vm in $(vboxmanage list vms | awk '{print $1}' | sed -e 's/\"//g')
do
 echo $vm 
 ${SCRIPTPATH}/vb $vm status
done
