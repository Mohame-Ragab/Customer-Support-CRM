import { useTranslation } from 'react-i18next'
import { Chip } from '@mui/material'
import { ContentType } from '../types'

const LABEL_KEYS: Record<ContentType, string> = {
  [ContentType.Faq]: 'knowledgeBase.type.faq',
  [ContentType.Article]: 'knowledgeBase.type.article',
  [ContentType.SolutionGuide]: 'knowledgeBase.type.solutionGuide',
}

interface ContentTypeChipProps {
  type: ContentType
}

export function ContentTypeChip({ type }: ContentTypeChipProps) {
  const { t } = useTranslation()
  return <Chip size="small" label={t(LABEL_KEYS[type])} />
}
