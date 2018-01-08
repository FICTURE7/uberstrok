import os
import sys
import shutil
import hashlib

def md5(path):
    hasher = hashlib.md5()
    with open(path, 'rb') as f:
        while True:
            data = f.read(4096)
            if not data:
                break
            hasher.update(data)
    return hasher.hexdigest()


def main(args):
    # Absolute path to the /scripts directory which contains this script.
    scripts_dir = os.path.realpath(os.path.dirname(__file__))
    logs_path = os.path.join(scripts_dir, 'postbuild.log')

    with open(logs_path, 'w') as logf:
        def log(line):
            logf.write(line + '\n')
            print(line)

        log(' - post build script running...')
        log(' - ----------------------------')

        solution_dir = os.path.join(scripts_dir, '../')
        solution_dir = os.path.realpath(solution_dir)

        log(' - solution directory -> {0}'.format(solution_dir))

        target = args[1]
        target_out_dir = args[2]
        target_dir = os.path.realpath(os.path.join(solution_dir, 'src', target, target_out_dir))

        log(' - target directory -> {0}'.format(target_dir))

        if not os.path.exists(target_dir):
            log(' - ** target directory does not exists!')
            return 1
        
        dst_dir = os.path.join(solution_dir, 'photon', 'deploy', target, 'bin')
        
        log(' - destination directory -> {0}'.format(dst_dir))

        if not os.path.exists(dst_dir):
            log(' - * destionation directory does not exists -> creating it')
            os.makedirs(dst_dir)

        files = os.listdir(target_dir)
        for f in files:
            src = os.path.join(target_dir, f)
            dst = os.path.join(dst_dir, f)

            if os.path.exists(dst):
                src_hash = md5(src)
                dst_hash = md5(dst)

                if dst_hash == src_hash:
                    log(' - ignoring {0} already up to date'.format(f))
                    continue

            log(' - copying {0}'.format(f))

            # Keep the metadata so photon is happier when watching files.
            shutil.copy2(src, dst_dir)

if __name__ == "__main__":
    main(sys.argv)
