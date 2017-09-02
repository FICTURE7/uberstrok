import os
import sys
import glob
import shutil

def main(args):
    name = args[1]
    src = args[2]
    dst = os.path.join(os.getcwd(), "../../../../photon/deploy")
    dst = os.path.join(dst, name, 'bin')

    files = glob.glob("*")
    for f in files:
        shutil.copy2(f, dst)

if __name__ == "__main__":
    main(sys.argv)