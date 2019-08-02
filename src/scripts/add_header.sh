#!/bin/bash
# License: MIT
# see LICENSE file in this folder, if this file is not preset refer to
# https://github.com/sprintworx-gmbh/examples/blob/master/LICENSE
# https://opensource.org/licenses/MIT
# https://mit-license.org/
#
# The following copyright applies
#
# Copyright © 2019 SprintWORX GmbH, authors: Matthias Fritsch and Daniel Bişar 
#
# <>< he is alive

# ATTENTION: we take no responsability for any damages done by this script
# make backups before

# this scripts adds/updates the following header to your source code files
# this script requires bash > 4.0

# modify the header according to your needs
HEADER=" License: MIT
 see LICENSE file in this folder, if this file is not preset refer to
 https://github.com/sprintworx-gmbh/examples/blob/master/LICENSE
 https://opensource.org/licenses/MIT
 https://mit-license.org/

 The following copyright applies

 Copyright © 2019 SprintWORX GmbH, authors: Matthias Fritsch and Daniel Bişar 

 <>< he is alive"

# define the different strings that mark a comment in a specific file type
declare -A COMMENT_MARKER
COMMENT_MARKER=( ["sh"]="#" ["cs"]="//" ["c"]="//" ["cpp"]="//" ["h"]="//" ["hpp"]="//" )

FILES=()
IS_DRY_RUN=false
IS_VERBOSE=false

# parse arguments
while [[ $# -gt 0 ]]
do
    key="$1"
	
    case $key in
        --dry-run)
            IS_DRY_RUN=true
            shift
            ;;
        --help) 
            echo "Usage: add_header.sh [options] FILES"
            echo "--dry-run        just prints what would be done"       
            echo "--help           prints this help message"
            echo "--verbose, -v    verbose output (prints file content)"
            echo
            echo "example:"
            echo "./add_header.sh --dry-run *.cs"
            echo 
            echo "ATTENTION: we take no responsability for any damages done by running this script - make backups first!"
            echo "Copyright © 2019 SprintWORX GmbH, authors: Matthias Fritsch and Daniel Bişar"
            echo "License: MIT"
            exit 0
            ;;
        --verbose|-v)
            IS_VERBOSE=true
            shift
            ;;
        *) # everything else
            FILES+=("$1")
            shift
            ;;
  esac
done

function escapeForSed()
{
    sed -e 's/\//\\\//g' <<< "$1";
}

for file in "${FILES[@]}"; do

    # ignore directories
    if [ -d "$file" ]; then
        continue
    fi

    name=$(basename -- "$file")
    extension="${name##*.}"
    comment_marker=""

    # if file extension is not empty
    if [ ! -z "$extension" ]; then
        comment_marker="${COMMENT_MARKER[$extension]}"
    fi

    escaped_marker=$(escapeForSed "$comment_marker")
    header=$(echo "$HEADER" | sed -e "s/^/$escaped_marker/g")    

    # for shell scripts we need to keep the first line (we expect shell scripts
    # to start with #!...) - add other file extensions as needed ( -o ... )
    if [ "$extension" = "sh" ]; then
        first_line=$(head -n 1 "$file")
        other_lines=$(tail -n +2 "$file")
        modified_file_content=$(echo "$first_line" && echo "$header" && echo "$other_lines")
    else
        file_content=$(cat "$file")
        modified_file_content=$(echo "$header" && echo && echo "$file_content")
    fi

    # dry run just prints the informations
    if [ "$IS_DRY_RUN" = "true" ]; then
        echo "File: $file"
        echo "Extension: $extension"
        echo "comment_marker: $comment_marker"
        echo "escaped_marker: $escaped_marker"
        [ "$IS_VERBOSE" = "true" ] && echo "$modified_file_content"
    else
        echo "$modified_file_content" > "$file"
    fi
done
