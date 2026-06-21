import { useEffect, useState } from "react";

/**
 * Mengembalikan versi "tertunda" dari `value`, baru update setelah
 * `delayMs` tanpa perubahan lebih lanjut. Dipakai untuk search bar
 * Project List supaya tidak fetch ke API di setiap keystroke.
 */
export function useDebounce<T>(value: T, delayMs = 400): T {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const timeoutId = setTimeout(() => setDebounced(value), delayMs);
    return () => clearTimeout(timeoutId);
  }, [value, delayMs]);

  return debounced;
}
