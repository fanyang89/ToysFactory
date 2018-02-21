import os
import sys
import config
from push_mods import push
from pull_mods import pull


def help():
    print('eg. Type "mm pull" or "mm" to sync with remote.')
    print('eg. Type "mm push" push mods to remote.')


def main():
    # 创建临时文件夹
    if not os.path.isdir(config.tmp_dir):
        os.mkdir(config.tmp_dir)

    if len(sys.argv) == 1:
        pull()
        return

    if len(sys.argv) >= 2:
        help()
        return

    cmd = sys.argv[1]
    if cmd == 'push':
        if not config.is_push_allowed:
            print("You don't have permission to push")
            return
        else:
            push()
            print('Done.')
            return
    elif cmd == 'help':
        help()
        return
    elif cmd == 'pull':
        pull()
        return
    else:
        print('Unknown command line arguments.')
        help()
        return


if __name__ == '__main__':
    main()
