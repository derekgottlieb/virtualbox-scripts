#!/bin/bash
# 2013-09-10: Derek Gottlieb

vboxmanage list vms | awk '{print $1}' | sed -e 's/\"//g'
