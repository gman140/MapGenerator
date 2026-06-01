let _running = false;
let _audio = null;
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
    window._runnerKeyHandler = function (e) {
        if (e.code === 'Space') {
            e.preventDefault();
            dotnetRef.invokeMethodAsync('JumpKey');
        } else if (e.code === 'Enter' || e.code === 'NumpadEnter') {
            e.preventDefault();
            dotnetRef.invokeMethodAsync('AttackKey');
        } else if (e.code === 'ShiftLeft' || e.code === 'ShiftRight' || e.code === 'KeyX') {
            e.preventDefault();
            dotnetRef.invokeMethodAsync('DodgeKey');
        }
    };
    document.addEventListener('keydown', window._runnerKeyHandler);
}

export function removeKeyHandler() {
    if (window._runnerKeyHandler) {
        document.removeEventListener('keydown', window._runnerKeyHandler);
        window._runnerKeyHandler = null;
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
        switch (c.t) {
            case 'fill':
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.fillStyle = c.c;
                ctx.fillRect(c.x, c.y, c.w, c.h);
                ctx.globalAlpha = 1;
                break;

            case 'text':
                ctx.save();
                ctx.font = c.f ?? '12px monospace';
                ctx.fillStyle = c.c;
                ctx.textAlign = c.al ?? 'left';
                ctx.fillText(c.s, c.x, c.y);
                ctx.restore();
                break;

            case 'bar':
                ctx.fillStyle = '#333';
                ctx.fillRect(c.x, c.y, c.w, c.h);
                ctx.fillStyle = c.c;
                ctx.fillRect(c.x, c.y, c.w * c.pct, c.h);
                break;

            case 'sprite': {
                const img = _sprites[c.s];
                if (img) {
                    ctx.save();
                    ctx.globalAlpha = c.alpha ?? 1;
                    ctx.imageSmoothingEnabled = false;
                    ctx.drawImage(img, c.x, c.y, c.w, c.h);
                    ctx.restore();
                }
                break;
            }

            case 'arc':
                ctx.save();
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.strokeStyle = c.c;
                ctx.lineWidth = c.h || 3;
                ctx.lineCap = 'round';
                ctx.beginPath();
                ctx.arc(c.x, c.y, c.w, c.a1, c.a2);
                ctx.stroke();
                ctx.restore();
                break;

            case 'poly': {
                const pts = c.pts;
                if (!pts || pts.length < 4) break;
                ctx.save();
                ctx.globalAlpha = c.alpha ?? 1;
                ctx.beginPath();
                ctx.moveTo(pts[0], pts[1]);
                for (let i = 2; i < pts.length; i += 2)
                    ctx.lineTo(pts[i], pts[i + 1]);
                ctx.closePath();
                if (c.c) { ctx.fillStyle = c.c; ctx.fill(); }
                ctx.restore();
                break;
            }
        }
    }
}

// ── Audio ─────────────────────────────────────────────────────────────────────

function getAudio() {
    if (!_audio) _audio = new (window.AudioContext || window.webkitAudioContext)();
    if (_audio.state === 'suspended') _audio.resume();
    return _audio;
}

export function playDamageSound() {
    try {
        const ctx = getAudio();
        // Short low-frequency thud with noise burst
        const len = Math.floor(ctx.sampleRate * 0.14);
        const buf = ctx.createBuffer(1, len, ctx.sampleRate);
        const data = buf.getChannelData(0);
        for (let i = 0; i < len; i++)
            data[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / len, 0.7);
        const src = ctx.createBufferSource();
        src.buffer = buf;
        const filt = ctx.createBiquadFilter();
        filt.type = 'lowpass';
        filt.frequency.value = 500;
        const g = ctx.createGain();
        g.gain.setValueAtTime(0.55, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.16);
        src.connect(filt); filt.connect(g); g.connect(ctx.destination);
        src.start();
    } catch (_) {}
}

export function playDodgeSound() {
    try {
        const ctx = getAudio();
        // Quick descending whoosh
        const osc = ctx.createOscillator();
        const g   = ctx.createGain();
        osc.connect(g); g.connect(ctx.destination);
        osc.type = 'sine';
        osc.frequency.setValueAtTime(900, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(220, ctx.currentTime + 0.14);
        g.gain.setValueAtTime(0.18, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.14);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.14);
    } catch (_) {}
}

export function playEnemyAttackSound() {
    try {
        const ctx = getAudio();
        // Heavy bandpass noise burst — heavier than the player attack
        const len = Math.floor(ctx.sampleRate * 0.10);
        const buf = ctx.createBuffer(1, len, ctx.sampleRate);
        const data = buf.getChannelData(0);
        for (let i = 0; i < len; i++)
            data[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / len, 1.1);
        const src  = ctx.createBufferSource();
        src.buffer = buf;
        const filt = ctx.createBiquadFilter();
        filt.type = 'bandpass';
        filt.frequency.value = 700;
        filt.Q.value = 0.7;
        const g = ctx.createGain();
        g.gain.setValueAtTime(0.50, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.12);
        src.connect(filt); filt.connect(g); g.connect(ctx.destination);
        src.start();
    } catch (_) {}
}

export function playJumpSound() {
    try {
        const ctx = getAudio();
        const osc = ctx.createOscillator();
        const g = ctx.createGain();
        osc.connect(g);
        g.connect(ctx.destination);
        osc.type = 'square';
        osc.frequency.setValueAtTime(180, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(540, ctx.currentTime + 0.10);
        g.gain.setValueAtTime(0.20, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.14);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.14);
    } catch (_) {}
}

export function playAttackSound() {
    try {
        const ctx = getAudio();
        const len = Math.floor(ctx.sampleRate * 0.09);
        const buf = ctx.createBuffer(1, len, ctx.sampleRate);
        const data = buf.getChannelData(0);
        for (let i = 0; i < len; i++)
            data[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / len, 1.5);

        const src = ctx.createBufferSource();
        src.buffer = buf;

        const filt = ctx.createBiquadFilter();
        filt.type = 'bandpass';
        filt.frequency.value = 1600;
        filt.Q.value = 1.8;

        const g = ctx.createGain();
        g.gain.setValueAtTime(0.45, ctx.currentTime);
        g.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.12);

        src.connect(filt);
        filt.connect(g);
        g.connect(ctx.destination);
        src.start();
    } catch (_) {}
}
