export function mountAndInitializeDb() {
    FS.mkdir('/database');
    FS.mount(IDBFS, {}, '/database');
    return syncDatabase();
}

export function syncDatabase() {

    return new Promise((resolve, reject) => {
        FS.syncfs((err) => {
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
