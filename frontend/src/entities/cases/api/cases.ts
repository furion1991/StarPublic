import { API } from '@/shared/api'

import type { Case, CasesResponse, GetAllCasesProps } from './cases.types'

export const getAllCases = async ({ page, pageItems }: GetAllCasesProps) => {
  const { data } = await API.get<CasesResponse>('/cases/getall', {
    params: {
      Page: page,
      Count: pageItems
    }
  })

  return data
}

export const getCase = async (id: string) => {
  const { data } = await API.get<Case>(`/cases/get/${id}`)

  return data
}

export const getUserFavoriteCase = async (userId: string) => {
  const { data } = await API.get<Case>(`/audit/favcase/${userId}`)

  return data
}
