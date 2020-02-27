#!/bin/bash

# Be smart. Be safe.
set -e

# Move to script's directory
cd "`dirname "$0"`"

# Actual thing
for SRC in `find Content -depth`
do
    DST=`dirname "${SRC}"`/`basename "${SRC}" | tr '[A-Z]' '[a-z]'`
    if [ "${SRC}" != "${DST}" ]
    then
        [ ! -e "${DST}" ] && mv -T "${SRC}" "${DST}" || echo "${SRC} was not renamed"
    fi
done

mv content Content
mv Content/ships Content/Ships
mv Content/Ships/hulktex1_1.xnb Content/Ships/hulkTex1_1.xnb
mv Content/meshes/metaltexture_1.xnb Content/meshes/metalTexture_1.xnb
mv Content/meshes/planettexture_1.xnb Content/meshes/planetTexture_1.xnb
mv Content/meshes/rocktexture_1.xnb Content/meshes/rockTexture_1.xnb
mv Content/projectiles/torpedotexture_1.xnb Content/projectiles/torpedoTexture_1.xnb
