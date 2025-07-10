import { API } from '@/shared/api'

import { OpenCaseData, type OpenCaseProps } from './cases.types'

export const openCase = ({ userId, caseId }: OpenCaseProps) => {
  return API.post<OpenCaseData>('/case/open', {
    userId,
    caseId
  })
}
