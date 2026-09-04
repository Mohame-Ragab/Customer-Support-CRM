import { useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, List, ListItem, ListItemText } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { downloadCustomerAttachment } from './api'
import { useCustomerAttachments, useUploadCustomerAttachment } from './useCustomerAttachments'

interface CustomerAttachmentsPanelProps {
  customerId: string
}

function formatSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

/** File attachments on a customer (F01 customers/manage-customer-attachments; minimal frontend added by F04's ticket-workspace customer panel). */
export function CustomerAttachmentsPanel({ customerId }: CustomerAttachmentsPanelProps) {
  const { t } = useTranslation()
  const {
    data: attachments,
    isLoading,
    isError,
    error,
    refetch,
  } = useCustomerAttachments(customerId)
  const uploadMutation = useUploadCustomerAttachment(customerId)
  const fileInputRef = useRef<HTMLInputElement | null>(null)
  const [uploadError, setUploadError] = useState<string | null>(null)

  const handleFileSelected = (file: File | undefined) => {
    if (!file) return
    setUploadError(null)
    uploadMutation.mutate(file, {
      onError: (err) => setUploadError(normalizeApiError(err).detail ?? t('errors.unexpected')),
    })
  }

  const handleDownload = (attachmentId: string, fileName: string) => {
    downloadCustomerAttachment(customerId, attachmentId, fileName).catch((err) =>
      setUploadError(normalizeApiError(err).detail ?? t('errors.unexpected')),
    )
  }

  return (
    <Box>
      {isLoading && <LoadingState />}
      {isError && (
        <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
      )}
      {!isLoading && !isError && (attachments?.length ?? 0) === 0 && (
        <EmptyState
          title={t('customers.attachments.empty', { defaultValue: 'No attachments yet' })}
        />
      )}
      {!isLoading && !isError && (attachments?.length ?? 0) > 0 && (
        <List>
          {(attachments ?? []).map((attachment) => (
            <ListItem
              key={attachment.id}
              divider
              secondaryAction={
                <Button
                  size="small"
                  onClick={() => handleDownload(attachment.id, attachment.fileName)}
                >
                  {t('customers.attachments.download', { defaultValue: 'Download' })}
                </Button>
              }
            >
              <ListItemText
                primary={attachment.fileName}
                secondary={formatSize(attachment.sizeBytes)}
              />
            </ListItem>
          ))}
        </List>
      )}

      {uploadError && (
        <Alert severity="error" sx={{ mt: 1 }}>
          {uploadError}
        </Alert>
      )}

      <Box sx={{ mt: 2 }}>
        <input
          ref={fileInputRef}
          type="file"
          hidden
          onChange={(e) => handleFileSelected(e.target.files?.[0])}
        />
        <Button
          size="small"
          variant="contained"
          disabled={uploadMutation.isPending}
          onClick={() => fileInputRef.current?.click()}
        >
          {t('customers.attachments.upload', { defaultValue: 'Upload file' })}
        </Button>
      </Box>
    </Box>
  )
}
