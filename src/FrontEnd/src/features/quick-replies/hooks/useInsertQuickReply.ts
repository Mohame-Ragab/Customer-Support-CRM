/**
 * Reusable insertion helper for reply composers (Email, Live Chat). Inserts
 * `body` at the current caret position of a textarea/input, updates the
 * selection to sit after the inserted text, and dispatches a synthetic
 * `input` event so a React-controlled component (whose value is driven by
 * `onChange`) re-renders with the new value.
 */
export function insertQuickReplyIntoTextarea(
  el: HTMLTextAreaElement | HTMLInputElement,
  body: string,
): void {
  const start = el.selectionStart ?? el.value.length
  const end = el.selectionEnd ?? el.value.length
  const before = el.value.slice(0, start)
  const after = el.value.slice(end)

  const nativeSetter = Object.getOwnPropertyDescriptor(
    el instanceof HTMLTextAreaElement ? HTMLTextAreaElement.prototype : HTMLInputElement.prototype,
    'value',
  )?.set

  const nextValue = `${before}${body}${after}`
  if (nativeSetter) {
    nativeSetter.call(el, nextValue)
  } else {
    el.value = nextValue
  }

  const caret = start + body.length
  el.selectionStart = caret
  el.selectionEnd = caret

  el.dispatchEvent(new Event('input', { bubbles: true }))
  el.focus()
}
