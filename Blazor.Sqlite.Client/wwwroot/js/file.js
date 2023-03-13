export function mountAndInitializeDb() {
    try {
        window.Module.FS.mkdir('/database');
        window.Module.FS.mount(window.Module.FS.filesystems.MEMFS, {}, '/database');
        return syncDatabase(true);
    } catch (ex) {
        console.error(ex);
    }
}

export function syncDatabase(populate) {

    return new Promise((resolve, reject) => {
        window.Module.FS.syncfs(populate, (err) => {
            if (err) {
                console.log('syncfs failed. Error:', err);
                reject(err);
            }
            else {
                console.log('synced successfull.');
                resolve();
            }
        });
    });
}
