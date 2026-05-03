window.gameJs = (() => {
    const BROWSER_ID_KEY = 'mapgen_browser_id';

    let _canvas = null;
    let _ctx = null;
    let _baseImageData = null; // minimap pixels without the player dot
    let _playerQ = 0;
    let _playerR = 0;

    function getBrowserId() {
        let id = localStorage.getItem(BROWSER_ID_KEY);
        if (!id) {
            id = crypto.randomUUID();
            localStorage.setItem(BROWSER_ID_KEY, id);
        }
        return id;
    }

    function clearBrowserId() {
        localStorage.removeItem(BROWSER_ID_KEY);
    }

    function drawMinimap(canvasEl, rgbBytes, width, height, playerQ, playerR) {
        if (!canvasEl) return;

        _canvas = canvasEl;
        _ctx = canvasEl.getContext('2d');
        _playerQ = playerQ;
        _playerR = playerR;

        canvasEl.width = width;
        canvasEl.height = height;

        // Build base ImageData from RGB byte array
        const imageData = _ctx.createImageData(width, height);
        const d = imageData.data;
        for (let i = 0; i < width * height; i++) {
            d[i * 4]     = rgbBytes[i * 3];
            d[i * 4 + 1] = rgbBytes[i * 3 + 1];
            d[i * 4 + 2] = rgbBytes[i * 3 + 2];
            d[i * 4 + 3] = 255;
        }
        _ctx.putImageData(imageData, 0, 0);

        // Keep a clean copy so we can erase the old dot before drawing a new one
        _baseImageData = _ctx.getImageData(0, 0, width, height);

        _drawPlayerDot(playerQ, playerR);
    }

    function updateMinimapPlayer(q, r) {
        if (!_ctx || !_baseImageData) return;

        // Restore clean base, then draw dot at new position
        _ctx.putImageData(_baseImageData, 0, 0);
        _playerQ = q;
        _playerR = r;
        _drawPlayerDot(q, r);
    }

    function _drawPlayerDot(q, r) {
        if (!_ctx) return;
        _ctx.fillStyle = '#f0e050';
        _ctx.strokeStyle = '#0e0e12';
        _ctx.lineWidth = 0.5;
        _ctx.beginPath();
        _ctx.arc(q, r, 2.5, 0, Math.PI * 2);
        _ctx.fill();
        _ctx.stroke();
    }

    function initMinimap(playerQ, playerR) {
        _playerQ = playerQ;
        _playerR = playerR;
    }

    return { getBrowserId, clearBrowserId, drawMinimap, updateMinimapPlayer, initMinimap };
})();
