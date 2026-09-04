import { useTranslation } from 'react-i18next'
import { IconButton, List, ListItem, ListItemText, Stack, Tooltip, Typography } from '@mui/material'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import { EmptyState } from '@/components/ui/EmptyState'
import type { QuickReplyTemplate } from '../types/quickReply'

interface QuickReplyListProps {
  templates: QuickReplyTemplate[]
  onEdit: (template: QuickReplyTemplate) => void
  onDelete: (template: QuickReplyTemplate) => void
}

export function QuickReplyList({ templates, onEdit, onDelete }: QuickReplyListProps) {
  const { t } = useTranslation()

  if (templates.length === 0) {
    return <EmptyState title={t('quickReplies.emptyState')} />
  }

  return (
    <List>
      {templates.map((template) => (
        <ListItem
          key={template.id}
          divider
          secondaryAction={
            <Stack direction="row" spacing={0.5}>
              <Tooltip title={t('quickReplies.edit')}>
                <IconButton edge="end" size="small" onClick={() => onEdit(template)}>
                  <EditIcon fontSize="small" />
                </IconButton>
              </Tooltip>
              <Tooltip title={t('quickReplies.delete')}>
                <IconButton edge="end" size="small" onClick={() => onDelete(template)}>
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            </Stack>
          }
        >
          <ListItemText
            primary={template.name}
            secondary={
              <Typography
                component="span"
                variant="body2"
                color="text.secondary"
                noWrap
                sx={{ display: 'block', maxWidth: 480 }}
              >
                {template.body}
              </Typography>
            }
          />
        </ListItem>
      ))}
    </List>
  )
}
