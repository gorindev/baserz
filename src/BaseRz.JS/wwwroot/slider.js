// Pointer capture for BaseSlider. The thumb's keyboard behavior stays in C#.
export function attach(track, dotNet, orientation) {
    if (!track) {
        return { dispose() {} };
    }

    const report = (event) => {
        const rect = track.getBoundingClientRect();
        const vertical = orientation === "vertical";
        const ratio = vertical
            ? 1 - ((event.clientY - rect.top) / (rect.height || 1))
            : (event.clientX - rect.left) / (rect.width || 1);
        const clamped = Math.min(1, Math.max(0, ratio));
        return dotNet.invokeMethodAsync("SetRatio", clamped);
    };

    const onPointerDown = (event) => {
        track.setPointerCapture?.(event.pointerId);
        report(event);
    };

    const onPointerMove = (event) => {
        if (track.hasPointerCapture && !track.hasPointerCapture(event.pointerId)) {
            return;
        }

        report(event);
    };

    const onPointerUp = (event) => {
        if (track.hasPointerCapture?.(event.pointerId)) {
            track.releasePointerCapture(event.pointerId);
        }
    };

    track.addEventListener("pointerdown", onPointerDown);
    track.addEventListener("pointermove", onPointerMove);
    track.addEventListener("pointerup", onPointerUp);

    return {
        dispose() {
            track.removeEventListener("pointerdown", onPointerDown);
            track.removeEventListener("pointermove", onPointerMove);
            track.removeEventListener("pointerup", onPointerUp);
        },
    };
}
