window.gameJs = (() => {
    const BROWSER_ID_KEY = 'mapgen_browser_id';

    let _canvas = null;
    let _ctx = null;
    let _baseImageData = null; // minimap pixels without the player dot
    let _playerQ = 0;
    let _playerR = 0;

    function generateUUID() {
        if (crypto.randomUUID) return crypto.randomUUID();
        // Fallback for non-secure contexts (plain HTTP on LAN)
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))).toString(16)
        );
    }

    function getBrowserId() {
        let id = localStorage.getItem(BROWSER_ID_KEY);
        if (!id) {
            id = generateUUID();
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

        const imageData = _ctx.createImageData(width, height);
        const d = imageData.data;
        for (let i = 0; i < width * height; i++) {
            d[i * 4]     = rgbBytes[i * 3];
            d[i * 4 + 1] = rgbBytes[i * 3 + 1];
            d[i * 4 + 2] = rgbBytes[i * 3 + 2];
            d[i * 4 + 3] = 255;
        }
        _ctx.putImageData(imageData, 0, 0);

        _baseImageData = _ctx.getImageData(0, 0, width, height);
        _drawPlayerDot(_ctx, playerQ, playerR, 2.5);
    }

    function updateMinimapPlayer(q, r) {
        if (!_ctx || !_baseImageData) return;
        _ctx.putImageData(_baseImageData, 0, 0);
        _playerQ = q;
        _playerR = r;
        _drawPlayerDot(_ctx, q, r, 2.5);
    }

    function openMinimapPopup(popupCanvasEl, playerQ, playerR) {
        if (!popupCanvasEl || !_baseImageData) return;

        const srcW = _baseImageData.width;
        const srcH = _baseImageData.height;

        // Scale to fit within 800×600, preserving aspect ratio
        const maxW = 800, maxH = 600;
        const scale = Math.min(maxW / srcW, maxH / srcH);
        const dstW = Math.round(srcW * scale);
        const dstH = Math.round(srcH * scale);

        popupCanvasEl.width  = dstW;
        popupCanvasEl.height = dstH;

        const pCtx = popupCanvasEl.getContext('2d');
        pCtx.imageSmoothingEnabled = false;

        // Draw base image scaled up via an offscreen canvas
        const off = new OffscreenCanvas(srcW, srcH);
        const offCtx = off.getContext('2d');
        offCtx.putImageData(_baseImageData, 0, 0);
        pCtx.drawImage(off, 0, 0, dstW, dstH);

        // Draw player dot proportionally scaled
        _drawPlayerDot(pCtx, playerQ * scale, playerR * scale, 4 * scale);
    }

    function _drawPlayerDot(ctx, x, y, radius) {
        ctx.fillStyle   = '#f0e050';
        ctx.strokeStyle = '#0e0e12';
        ctx.lineWidth   = 0.5;
        ctx.beginPath();
        ctx.arc(x, y, radius, 0, Math.PI * 2);
        ctx.fill();
        ctx.stroke();
    }

    function initMinimap(playerQ, playerR) {
        _playerQ = playerQ;
        _playerR = playerR;
    }

    return { getBrowserId, clearBrowserId, drawMinimap, updateMinimapPlayer, openMinimapPopup, initMinimap };
})();
