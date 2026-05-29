let _running = false;
let _audio = null;
let _reelOsc = null;
let _reelGain = null;
const _sprites = {};

// ── Sprite pre-rendering ──────────────────────────────────────────────────────

export function buildSprite(key, pixels, cellSize) {
    const size = Math.round(Math.sqrt(pixels.length));
    const canvas = document.createElement('canvas');
    canvas.width  = size * cellSize;
    canvas.height = size * cellSize;
    const ctx = canvas.getContext('2d');
    ctx.imageSmoothingEnabled = false;
    for (let i = 0; i < pixels.length; i++) {
        if (!pixels[i]) continue;
        ctx.fillStyle = pixels[i];
        ctx.fillRect((i % size) * cellSize, Math.floor(i / size) * cellSize, cellSize, cellSize);
    }
    _sprites[key] = canvas;
}

export function clearSprites() {
    for (const k in _sprites) delete _sprites[k];
}

// ── Frame loop ────────────────────────────────────────────────────────────────

export function startLoop(dotnetRef) {
    _running = true;
    function frame(ts) {
        if (!_running) return;
        dotnetRef.invokeMethodAsync('OnFrame', ts);
        requestAnimationFrame(frame);
    }
    requestAnimationFrame(frame);
}

export function stopLoop() {
    _running = false;
}

// ── Keyboard ─────────────────────────────────────────────────────────────────

export function initKeyHandler(dotnetRef) {
    removeKeyHandler();
    window._fishingKeyDown = function (e) {
        if (e.code === 'Space') {
            e.preventDefault();
            if (!e.repeat)
                dotnetRef.invokeMethodAsync('HookKey');
        }
    };
    document.addEventListener('keydown', window._fishingKeyDown);
}

export function removeKeyHandler() {
    if (window._fishingKeyDown) {
        document.removeEventListener('keydown', window._fishingKeyDown);
        window._fishingKeyDown = null;
    }
}

// ── Canvas rendering ──────────────────────────────────────────────────────────

export function renderFrame(canvasId, commandsJson) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const cmds = JSON.parse(commandsJson);

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    for (const c of cmds) {
        ctx.save();
        switch (c.t) {
            case 'fill':
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.fillStyle = c.c;
                ctx.fillRect(c.x, c.y, c.w, c.h);
                break;

            case 'text':
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.font = c.f ?? '12px monospace';
                ctx.fillStyle = c.c;
                ctx.textAlign = c.al ?? 'left';
                ctx.textBaseline = 'alphabetic';
                ctx.fillText(c.s, c.x, c.y);
                break;

            case 'bar':
                ctx.fillStyle = '#222';
                ctx.fillRect(c.x, c.y, c.w, c.h);
                ctx.fillStyle = c.c;
                ctx.fillRect(c.x, c.y, c.w * c.pct, c.h);
                break;

            case 'circle':
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.fillStyle = c.c;
                ctx.beginPath();
                ctx.arc(c.x, c.y, c.r, 0, Math.PI * 2);
                ctx.fill();
                break;

            case 'line':
                ctx.strokeStyle = c.c;
                ctx.lineWidth = c.alpha || 1.5;  // alpha field stores lineWidth for lines
                ctx.lineCap = 'round';
                ctx.beginPath();
                ctx.moveTo(c.x, c.y);
                ctx.lineTo(c.w, c.h);  // w=x2, h=y2
                ctx.stroke();
                break;

            case 'sprite': {
                const img = _sprites[c.s];
                if (img) {
                    ctx.imageSmoothingEnabled = false;
                    ctx.drawImage(img, c.x, c.y, c.w, c.h);
                }
                break;
            }

            case 'arc':
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.strokeStyle = c.c;
                ctx.lineWidth = c.h || 2;
                ctx.lineCap = 'round';
                ctx.beginPath();
                ctx.arc(c.x, c.y, c.w, c.a1, c.a2);
                ctx.stroke();
                break;
        }
        ctx.restore();
    }
}

// ── Audio ─────────────────────────────────────────────────────────────────────

function getAudio() {
    if (!_audio) _audio = new (window.AudioContext || window.webkitAudioContext)();
    if (_audio.state === 'suspended') _audio.resume();
    return _audio;
}

export function playHookSound() {
    try {
        const ctx = getAudio();
        const osc = ctx.createOscillator();
        const g   = ctx.createGain();
        osc.connect(g);
        g.connect(ctx.destination);
        osc.type = 'sine';
        osc.frequency.setValueAtTime(440, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(880, ctx.currentTime + 0.06);
        g.gain.setValueAtTime(0.25, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.18);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.18);
    } catch (_) {}
}

export function playCatchSound() {
    try {
        const ctx = getAudio();
        const freqs = [523, 659, 784, 1047];
        freqs.forEach((f, i) => {
            const osc = ctx.createOscillator();
            const g   = ctx.createGain();
            osc.connect(g);
            g.connect(ctx.destination);
            osc.type = 'sine';
            osc.frequency.value = f;
            const t = ctx.currentTime + i * 0.10;
            g.gain.setValueAtTime(0.20, t);
            g.gain.exponentialRampToValueAtTime(0.001, t + 0.25);
            osc.start(t);
            osc.stop(t + 0.25);
        });
    } catch (_) {}
}

export function playSnapSound() {
    try {
        const ctx = getAudio();
        const len = Math.floor(ctx.sampleRate * 0.08);
        const buf = ctx.createBuffer(1, len, ctx.sampleRate);
        const data = buf.getChannelData(0);
        for (let i = 0; i < len; i++)
            data[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / len, 2);
        const src = ctx.createBufferSource();
        src.buffer = buf;
        const g = ctx.createGain();
        g.gain.setValueAtTime(0.5, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.12);
        src.connect(g);
        g.connect(ctx.destination);
        src.start();
    } catch (_) {}
}

export function playTapHitSound() {
    try {
        const ctx = getAudio();
        const osc = ctx.createOscillator();
        const g   = ctx.createGain();
        osc.connect(g); g.connect(ctx.destination);
        osc.type = 'sine';
        osc.frequency.setValueAtTime(600, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(900, ctx.currentTime + 0.05);
        g.gain.setValueAtTime(0.18, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.12);
        osc.start(ctx.currentTime); osc.stop(ctx.currentTime + 0.12);
    } catch (_) {}
}

export function playTapMissSound() {
    try {
        const ctx = getAudio();
        const osc = ctx.createOscillator();
        const g   = ctx.createGain();
        osc.connect(g); g.connect(ctx.destination);
        osc.type = 'square';
        osc.frequency.setValueAtTime(160, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(80, ctx.currentTime + 0.08);
        g.gain.setValueAtTime(0.14, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.10);
        osc.start(ctx.currentTime); osc.stop(ctx.currentTime + 0.10);
    } catch (_) {}
}
