import qiniu
import os
import config
import progressbar
import utils
import json


def push():
    local_mods = utils.get_local_mods()
    remote_mods = utils.get_remote_mods()
    intersection = remote_mods & local_mods

    push_list = local_mods - intersection
    remove_list = remote_mods - intersection

    if not remove_list and not push_list:
        print('Nothing changed.')
        return

    q = qiniu.Auth(config.access_key, config.secret_key)
    bucket = qiniu.BucketManager(q)

    # 删除mod
    if not remove_list:
        print('Removing mods at remote.')
        bar = progressbar.ProgressBar()
        for mod_name in bar(remove_list):
            ret, info = bucket.delete(config.bucket_name, mod_name)

    # 上传mod
    print('Pushing mods to remote.')
    bar = progressbar.ProgressBar()
    for mod_name in bar(push_list):
        token = q.upload_token(config.bucket_name, mod_name)
        abspath = os.path.join(config.mod_dir, mod_name)
        ret, info = qiniu.put_file(token, mod_name, abspath)
        assert ret['hash'] == qiniu.etag(abspath)

    # 更新mod列表
    print('Updating mod index.')
    index_filename = 'index.json'
    index_path = os.path.join(config.tmp_dir, index_filename)
    with open(index_path, 'w') as fp:
        json.dump(list(local_mods), fp)
    token = q.upload_token(config.bucket_name, index_filename)
    ret, info = qiniu.put_file(token, index_filename, index_path)
