import { useEffect, useRef } from 'react'
import { useTranslation } from 'react-i18next'
import { TextField } from '@mui/material'

interface KnowledgeBaseSearchBoxProps {
  value: string
  onChange: (value: string) => void
  /** Fires 300ms after the user stops typing, and immediately on Enter. */
  onSearch: (value: string) => void
}

/** Debounced (300ms) search input; RTL-aware via the browser's native `dir="auto"`. */
export function KnowledgeBaseSearchBox({ value, onChange, onSearch }: KnowledgeBaseSearchBoxProps) {
  const { t } = useTranslation()
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null)

  const handleChange = (next: string) => {
    onChange(next)

    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }
    timeoutRef.current = setTimeout(() => onSearch(next), 300)
  }

  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current)
      }
    }
  }, [])

  return (
    <TextField
      fullWidth
      value={value}
      onChange={(e) => handleChange(e.target.value)}
      onKeyDown={(e) => {
        if (e.key === 'Enter') {
          if (timeoutRef.current) clearTimeout(timeoutRef.current)
          onSearch(value)
        }
      }}
      placeholder={t('knowledgeBase.search.placeholder')}
      slotProps={{ htmlInput: { dir: 'auto' } }}
    />
  )
}
