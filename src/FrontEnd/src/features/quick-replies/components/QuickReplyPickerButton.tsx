import { useState, type MouseEvent } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, ListItemText, Menu, MenuItem } from '@mui/material'
import { useQuickReplies } from '../hooks/useQuickReplies'

interface QuickReplyPickerButtonProps {
  /** Called with the selected template's body. Plain text - the composer decides how to insert it (e.g. at caret). */
  onPick: (body: string) => void
}

/**
 * Button + menu listing the agent's own quick-reply templates. This is the
 * surface reused by the Email (F03) and Live Chat (F03) reply composers - see
 * their integration points in EmailReplyComposer.tsx / CustomerChatPanel.tsx /
 * AgentChatWindow.tsx. Body is rendered/stored as plain text; if a composer
 * ever becomes contenteditable/HTML, that composer is responsible for
 * sanitising before insertion.
 */
export function QuickReplyPickerButton({ onPick }: QuickReplyPickerButtonProps) {
  const { t } = useTranslation()
  const { data: templates } = useQuickReplies()
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null)

  const handleOpen = (event: MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget)
  const handleClose = () => setAnchorEl(null)

  return (
    <>
      <Button size="small" variant="outlined" onClick={handleOpen}>
        {t('quickReplies.pickerButton')}
      </Button>
      <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={handleClose}>
        {(templates ?? []).length === 0 && (
          <MenuItem disabled>{t('quickReplies.emptyState')}</MenuItem>
        )}
        {(templates ?? []).map((template) => (
          <MenuItem
            key={template.id}
            onClick={() => {
              onPick(template.body)
              handleClose()
            }}
          >
            <ListItemText primary={template.name} />
          </MenuItem>
        ))}
      </Menu>
    </>
  )
}
