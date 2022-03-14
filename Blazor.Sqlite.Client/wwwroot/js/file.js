export function mount(ref) {
    FS.mkdir('/database');
    FS.mount(IDBFS, {}, '/database');
    FS.syncfs(true, function (err) {
        if (!!err) {
            console.log('syncfs failed. Error:', err);
            ref.invokeMethodAsync('FinishSync', false);
        }
        else {
            console.log('synced successfull.');
            ref.invokeMethodAsync('FinishSync', true);
        }
    });
}

export function sync() {
    FS.syncfs(function (err) {
        if (err) {
            console.log('syncfs failed. Error:', err);
        }
        else {
            console.log('synced successfull.');
        }
    });
}
