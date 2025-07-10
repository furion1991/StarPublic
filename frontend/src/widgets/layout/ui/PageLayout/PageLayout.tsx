'use client'

import { Suspense } from 'react'

import { Header } from '@/widgets/header'
import { Footer } from '@/widgets/footer'
import { MainLoadingIndicator } from '@/shared/ui'
import { MobileNav } from '../MobileNav/MobileNav'

import { useUser } from '@/shared/hooks'

import classes from './PageLayout.module.scss'

type PageLayoutProps = {
  children: React.ReactNode
}

export const PageLayout = ({ children }: PageLayoutProps) => {
  const { isUserLoading } = useUser()

  return (
    <div className={classes.pageLayout}>
      <Header />

      <Suspense fallback={<MainLoadingIndicator />}>
        <main>{!isUserLoading ? children : <MainLoadingIndicator />}</main>
      </Suspense>

      <MobileNav />

      <Footer />
    </div>
  )
}
