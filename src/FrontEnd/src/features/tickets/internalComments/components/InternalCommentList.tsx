import { useTranslation } from 'react-i18next'
import { List, ListItem, ListItemText, Typography } from '@mui/material'
import type { TicketInternalComment } from '../types'

interface InternalCommentListProps {
  comments: TicketInternalComment[]
}

export function InternalCommentList({ comments }: InternalCommentListProps) {
  const { t } = useTranslation()

  return (
    <List>
      {comments.map((comment) => (
        <ListItem key={comment.id} divider alignItems="flex-start">
          <ListItemText
            primary={
              <Typography component="span" variant="subtitle2">
                {comment.authorDisplayName ??
                  t('ticketInternalComments.deletedUser', { defaultValue: 'Deleted user' })}
                {' · '}
                <Typography component="span" variant="caption" color="text.secondary">
                  {new Date(comment.createdAt).toLocaleString()}
                </Typography>
              </Typography>
            }
            secondary={
              <Typography
                component="span"
                variant="body2"
                sx={{ whiteSpace: 'pre-wrap', display: 'block' }}
                dir="auto"
              >
                {comment.body}
              </Typography>
            }
          />
        </ListItem>
      ))}
    </List>
  )
}
