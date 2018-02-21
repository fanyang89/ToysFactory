import os
import qiniu
import config
import requests
import json
import glob


def search_mod_dir():
    possible_dirs = [
        './../.minecraft/mods',
        './../../.minecraft/mods',
        './.minecraft/mods'
    ]
    results = [dir for dir in possible_dirs if os.path.isdir(dir)]
    if not results:
        raise Exception('Minecraft mod dir not found.')
    if len(results) > 1:
        raise Exception('More than one Minecraft mod dir found.')
    return os.path.abspath(results[0])


def get_local_mods():
    postfixes = ['*.jar', '*.litemod']
    patterns = [os.path.join(config.mod_dir, it) for it in postfixes]
    mod_abspath_list = sum([glob.glob(it, recursive=False)
                            for it in patterns], [])
    return set([os.path.basename(it) for it in mod_abspath_list])


def get_remote_mods():
    q = qiniu.Auth(config.access_key, config.secret_key)
    info_url = 'http://{}/{}'.format(config.base_uri, 'index.json')
    private_url = q.private_download_url(info_url)
    response = requests.get(private_url)
    assert response.status_code == 200
    if response.status_code != 200:
        raise Exception('Failed to get remote mod list.')
    return set(json.loads(response.text))
