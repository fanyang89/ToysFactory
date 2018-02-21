import qiniu
import os
import config
import progressbar
import utils
import requests


def download_file(url, path, filename):
    req = requests.get(url)
    with open(os.path.join(path, filename), 'wb') as f:
        for chunk in req.iter_content(chunk_size=1024):
            if chunk:
                f.write(chunk)
    return


def pull():
    local_mods = utils.get_local_mods()
    remote_mods = utils.get_remote_mods()
    intersection = remote_mods & local_mods

    pull_list = remote_mods - intersection
    remove_list = local_mods - intersection

    if not remove_list and not pull_list:
        print('Nothing changed.')
        return

    q = qiniu.Auth(config.access_key, config.secret_key)
    bucket = qiniu.BucketManager(q)

    # 删除mod
    if remove_list:
        print('Remove local useless mods.')
        bar = progressbar.ProgressBar()
        for mod_name in bar(remove_list):
            abspath = os.path.join(config.mod_dir, mod_name)
            os.remove(abspath)

    # 下载mod
    if pull_list:
        print('Downloading mods from remote.')
        bar = progressbar.ProgressBar()
        for mod_name in bar(pull_list):
            download_path = os.path.join(config.tmp_dir, mod_name)
            base_url = 'http://{}/{}'.format(config.base_uri, mod_name)
            private_url = q.private_download_url(base_url)
            download_file(private_url, config.tmp_dir, mod_name)
            ret, info = bucket.stat(config.bucket_name, mod_name)
            assert ret['hash'] == qiniu.etag(download_path)
            os.rename(download_path, os.path.join(config.mod_dir, mod_name))
