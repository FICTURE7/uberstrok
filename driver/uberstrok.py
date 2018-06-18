import argparse

def install():
    pass

def update():
    pass

def start():
    pass

def main():
    parser = argparse.ArgumentParser(description='uberstrok driver, which helps with the maintenance of the emulator')
    subparsers = parser.add_subparsers(title='subcommands', description='subcommands to use maintain the emulator', dest='subcommand')

    parser_install = subparsers.add_parser('install', help='installs uberstrok')
    parser_update = subparsers.add_parser('update', help='updates uberstrok')
    parser_start = subparsers.add_parser('start', help='starts uberstrok')
    args = parser.parse_args()

if __name__ == '__main__':
    main()
