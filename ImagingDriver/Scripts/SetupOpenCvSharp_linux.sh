#!/bin/bash
ORIG_DIR=$(pwd)
mkdir -p "/home/${USER}/source/repos"
cd "/home/${USER}/source/repos"
git clone https://github.com/shimat/opencvsharp.git
cd opencvsharp/src
mkdir build
cd build
cmake -D CMAKE_INSTALL_PREFIX=/usr/local ..
make -j 4
sudo make install
cd ~
sudo ldconfig
cd "${ORIG_DIR}"
