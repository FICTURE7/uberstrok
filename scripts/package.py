import os
import zipfile
import argparse
from glob import glob

class Packager(object):
    """
    represents a packager which
    packages a part of the final package
    """
    def __init__(self, src, name):
        self.__name = name
        self.__path = os.path.join(src, name, "bin", "Release")

    def pack(self, archive):
        """carries out the packing process

        method which should be overridden to implement the actual
        packing process

        returns true if sucess; otherwise; false
        """
        print(" packing {0} at: \n\t{1}".format(self.name, self.path))
        if not os.path.exists(self.path):
            print("\terr: ***directory does not exists")
            print("\t     ***make sure the project has been built in 'Release' mode")
            return False
        return True
    @property
    def name(self):
        return self.__name

    @property
    def path(self):
        return self.__path

class CommServerPackager(Packager):
    """ 
    represents a packager which
    packages the comm server
    """
    def __init__(self, src):
        Packager.__init__(self, src, "UberStrok.Realtime.Server.Comm")

    def pack(self, archive):
        if not Packager.pack(self, archive):
            return False
        files = glob(os.path.join(self.path, "*.dll"))
        for f in files:
            nf = os.path.join(self.name, os.path.basename(f))
            print("\tpacking {0}".format(nf))
            archive.write(f, arcname=nf)
        return True
    
class GameServerPackager(Packager):
    """ 
    represents a packager which
    packages the game server
    """
    def __init__(self,  src):
        Packager.__init__(self, src, "UberStrok.Realtime.Server.Game")

    def pack(self, archive):
        if not Packager.pack(self, archive):
            return False
        files = glob(os.path.join(self.path, "*.dll"))
        for f in files:
            nf = os.path.join(self.name, os.path.basename(f))
            print("\tpacking {0}".format(nf))
            archive.write(f, arcname=nf)
        return True

class WebServicesPackager(Packager):
    """
    represents a packager which
    packages the web services
    """
    def __init__(self, src):
        Packager.__init__(self, src, "UberStrok.WebServices")

    def pack(self, archive):
        if not Packager.pack(self, archive):
            return False

        types = ("*.exe", "*.dll", "*.config")
        files = []
        for t in types:
            files = glob(os.path.join(self.path, t))
            for f in files:
                if f.endswith(".vshost.exe"):
                    continue

                nf = os.path.join(self.name, os.path.basename(f))
                print("\tpacking {0}".format(nf))
                archive.write(f, arcname=nf)

        configs_path = os.path.join(self.path, "configs")
        configs_files = glob(os.path.join(configs_path, "**"), recursive=True)

        for f in configs_files[2:]:
            basedir = f[len(self.path) + 1:]
            nf = os.path.join(self.name, basedir)
            print("\tpacking {0}".format(nf))
            archive.write(f, arcname=nf)

        return True

class PatcherPackager(Packager):
    """
    represents a packager which
    packages the patcher
    """
    def __init__(self, src):
        Packager.__init__(self, src, "UberStrok.Patcher")

    def pack(self, archive):
        if not Packager.pack(self, archive):
            return False

        types = ("*.exe", "*.dll", "*.config")
        files = []
        for t in types:
            files = glob(os.path.join(self.path, t))
            for f in files:
                if f.endswith(".vshost.exe"):
                    continue

                nf = os.path.join(self.name, os.path.basename(f))
                print("\tpacking {0}".format(nf))
                archive.write(f, arcname=nf)
        return True

class Package(object):
    """
    represents the final package
    """
    def __init__(self, root, output):
        # root directory which should contain the
        # 'UberStrok.sln' file
        self.__root = os.path.abspath(root)        
        self.__archive = zipfile.ZipFile(output, mode="w")

        # 'src' directory containing the source
        src = os.path.join(self.__root, "src")
        
        self.__game_server_packager = GameServerPackager(src)
        self.__comm_server_packager = CommServerPackager(src)
        self.__webservices_packager = WebServicesPackager(src)
        self.__patcher_packager = PatcherPackager(src)

    def pack(self):
        print(" root -> {0}".format(self.__root))
        print(" -")
        self.__game_server_packager.pack(self.__archive)
        print(" -")
        self.__comm_server_packager.pack(self.__archive)
        print(" -")
        self.__webservices_packager.pack(self.__archive)
        print(" -")
        self.__patcher_packager.pack(self.__archive)

def main():
    # parse command line arguments
    parser = argparse.ArgumentParser(description="uberstrok emulator package script")
    parser.add_argument("root-path", nargs="?", help="path to the root directory", default="../")
    parser.add_argument("--output", help="path to the output archive", default="package.zip")
    args = vars(parser.parse_args())
    
    # parse arguments to the packager & pack
    package = Package(args["root-path"], args["output"])
    package.pack()

if __name__ == "__main__":
    main()
